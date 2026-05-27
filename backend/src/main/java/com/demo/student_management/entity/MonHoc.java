package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name = "MONHOC")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class MonHoc {

    @Id
    @Column(name = "ID_MONHOC", length = 20, nullable = false)
    @EqualsAndHashCode.Include
    private String idMonHoc;

    @Column(name = "TEN_MONHOC", length = 100, nullable = false)
    private String tenMonHoc;

    @Column(name = "TRANG_THAI_SU_DUNG")
    @Builder.Default
    private Boolean trangThaiSuDung = true;

    @OneToMany(mappedBy = "monHoc", fetch = FetchType.LAZY)
    @JsonIgnore
    @Builder.Default
    private List<ChiTietDiem> chiTietDiems = new ArrayList<>();
}