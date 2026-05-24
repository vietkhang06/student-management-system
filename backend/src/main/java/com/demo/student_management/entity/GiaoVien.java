package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "GIAOVIEN")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class GiaoVien {

    @Id
    @Column(name = "ID_GIAOVIEN", length = 10, nullable = false)
    @EqualsAndHashCode.Include
    private String idGiaoVien;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_LOP", nullable = false)
    private Lop lop;

    @OneToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_TAIKHOAN", nullable = false, unique = true)
    private TaiKhoan taiKhoan;

    @Column(name = "LUONG")
    private Integer luong;
}