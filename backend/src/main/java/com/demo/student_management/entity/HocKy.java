package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name = "HOCKY")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class HocKy {

    @Id
    @Column(name = "ID_HOCKY", length = 20, nullable = false)
    @EqualsAndHashCode.Include
    private String idHocKy;

    @Column(name = "TEN_HOCKY", length = 50)
    private String tenHocKy;

    @Column(name = "NAM_HOC", length = 10)
    private String namHoc;

    @OneToMany(mappedBy = "hocKy", fetch = FetchType.LAZY)
    @JsonIgnore
    @Builder.Default
    private List<ChiTietDiem> chiTietDiems = new ArrayList<>();
}